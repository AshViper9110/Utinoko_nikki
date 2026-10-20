
<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::create('play_logs', function (Blueprint $table) {
            $table->id();

            $table->unsignedBigInteger('elder_user_id');
            $table->unsignedBigInteger('game_id');

            $table->integer('score')->default(0);

            $table->dateTime('created_at')->useCurrent();
            $table->dateTime('updated_at')
                ->useCurrent()
                ->useCurrentOnUpdate();

            // 高齢者ユーザーとの外部キー制約
            $table->foreign('elder_user_id')
                ->references('id')
                ->on('elder_users');

            // ゲームとの外部キー制約
            $table->foreign('game_id')
                ->references('id')
                ->on('games');
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('play_logs');
    }
};
